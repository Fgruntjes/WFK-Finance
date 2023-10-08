locals {
  secret_name = ""
  backend_env = {
    Auth0__Domain = var.auth0_domain,
    //Auth0__Audience                      = var.auth0_audience,
    Nordigen__SecretId  = var.nordigen_secret_id,
    Nordigen__SecretKey = var.nordigen_secret_key,
    ConnectionStrings__DefaultConnection = join(";", [
      "Server=${azurerm_mssql_server.database.fully_qualified_domain_name}",
      "Database=${azurerm_mssql_database.backend.name}",
      "User Id=${azurerm_mssql_server.database.administrator_login}",
      "Password=${azurerm_mssql_server.database.administrator_login_password}",
    ])
  }
}

resource "azurerm_container_app_environment" "backend" {
  name                = "${var.app_environment}-Backend-Environment"
  location            = var.arm_location
  resource_group_name = var.app_project_slug
  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_user_assigned_identity" "app_backend" {
  name                = "app_backend"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "azurerm_role_assignment" "app_backend" {
  scope                = data.azurerm_container_registry.app.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.app_backend.principal_id
}

resource "azurerm_container_app" "backend" {
  name                         = "${var.app_environment}-backend"
  container_app_environment_id = azurerm_container_app_environment.backend.id
  resource_group_name          = var.app_project_slug
  revision_mode                = "Single"

  tags = {
    environment = var.app_environment
  }

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.app_backend.id
    ]
  }

  registry {
    server   = data.azurerm_container_registry.app.login_server
    identity = azurerm_user_assigned_identity.app_backend.id
  }

  template {
    container {
      name   = "app"
      image  = "${data.azurerm_container_registry.app.login_server}/${var.app_environment}/backend:${var.app_version}"
      cpu    = 0.5
      memory = "1Gi"

      dynamic "env" {
        for_each = local.backend_env
        content {
          name        = env.key
          secret_name = "appsettings"
        }
      }
    }
  }

  secret {
    name  = "appsettings"
    value = jsonencode(local.backend_env)
  }
}
