locals {
  backend_app_env = {
    Auth0__Domain = var.auth0_domain,
    //Auth0__Audience                      = var.auth0_audience,
    Nordigen__SecretId  = var.nordigen_secret_id,
    Nordigen__SecretKey = var.nordigen_secret_key,
    ConnectionStrings__DefaultConnection = join(";", [
      "Server=${azurerm_mssql_server.backend_database.fully_qualified_domain_name}",
      "Database=${azurerm_mssql_database.backend_database.name}",
      "Authentication=Active Directory Managed Identity",
      "Persist Security Info=False",
      "MultipleActiveResultSets=False",
      "Encrypt=True",
    ])
  }
}

resource "azurerm_container_app_environment" "backend_app" {
  name                = "${var.app_environment}-backend-app"
  location            = var.arm_location
  resource_group_name = var.app_project_slug

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_user_assigned_identity" "backend_app" {
  name                = "${var.app_environment}-backend-app"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "azurerm_role_assignment" "backend_app" {
  scope                = data.azurerm_container_registry.app.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.backend_app.principal_id
}

resource "mssql_user" "backend_app" {
  server {
    host = azurerm_mssql_server.backend_database.fully_qualified_domain_name
    azure_login {
      client_id     = var.arm_client_id
      tenant_id     = var.arm_tenant_id
      client_secret = var.arm_client_secret
    }
  }
  depends_on = [azurerm_mssql_firewall_rule.backend_database_public]

  database  = azurerm_mssql_database.backend_database.name
  username  = azurerm_user_assigned_identity.backend_app.name
  object_id = azurerm_user_assigned_identity.backend_app.client_id

  roles = ["db_datareader", "db_datawriter"]
}

resource "azurerm_container_app" "backend_app" {
  name                         = "${var.app_environment}-backend-app"
  container_app_environment_id = azurerm_container_app_environment.backend_app.id
  resource_group_name          = var.app_project_slug
  revision_mode                = "Single"

  tags = {
    environment = var.app_environment
  }

  ingress {
    external_enabled = true
    target_port      = 8080
    traffic_weight {
      # Keep @see https://github.com/hashicorp/terraform-provider-azurerm/issues/21022
      latest_revision = true
      percentage      = 100
    }
  }

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.backend_app.id
    ]
  }

  registry {
    server   = data.azurerm_container_registry.app.login_server
    identity = azurerm_user_assigned_identity.backend_app.id
  }

  secret {
    name  = "${var.app_environment}-backend-app-settings"
    value = jsonencode(local.backend_app_env)
  }

  template {
    container {
      name   = "app"
      image  = "${data.azurerm_container_registry.app.login_server}/${var.app_environment}/app.backend:${var.app_version}"
      cpu    = 0.5
      memory = "1Gi"


      dynamic "env" {
        for_each = local.backend_app_env
        content {
          name        = env.key
          secret_name = "${var.app_environment}-backend-app-settings"
        }
      }
    }
  }
}
