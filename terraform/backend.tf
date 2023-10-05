locals {
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

resource "azurerm_key_vault" "vault" {
  name                       = "${var.app_project_slug}-${var.app_environment}"
  location                   = var.arm_location
  resource_group_name        = var.app_project_slug
  tenant_id                  = var.arm_tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7

  access_policy {
    tenant_id = var.arm_tenant_id
    object_id = var.arm_client_id

    key_permissions = [
      "Create",
      "Get",
    ]

    secret_permissions = [
      "Set",
      "Get",
      "Delete",
      "Purge",
      "Recover"
    ]
  }

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_key_vault_secret" "backend" {
  name         = "${var.app_environment}-backend-secret"
  value        = jsonencode(local.backend_env)
  key_vault_id = azurerm_key_vault.vault.id
  tags = {
    environment = var.app_environment
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

resource "azurerm_container_app" "backend" {
  name                         = "${var.app_environment}-backend"
  container_app_environment_id = azurerm_container_app_environment.backend.id
  resource_group_name          = var.app_project_slug
  revision_mode                = "Single"
  tags = {
    environment = var.app_environment
  }

  template {
    container {
      name   = "app"
      image  = "${var.app_project_slug}.azurecr.io/${var.app_environment}/backend:${var.app_version}"
      cpu    = 1
      memory = "0.5Gi"

      env {
        secret_name = "Auth0__Domain"
        name        = "Auth0__Domain"
      }

      dynamic "env" {
        for_each = local.backend_env
        content {
          name        = env.key
          secret_name = azurerm_key_vault_secret.backend.name

        }
      }
    }
  }
}
