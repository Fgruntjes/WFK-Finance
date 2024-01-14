locals {
  backend_database_backups_enabled = var.app_environment == "main"
}

resource "random_password" "backend_database_sa_password" {
  count            = local.environment_data_ephemeral ? 1 : 0
  length           = 16
  special          = true
  min_lower        = 1
  min_special      = 1
  min_numeric      = 1
  override_special = "_%@"
}

resource "azurerm_mssql_server" "backend_database" {
  name                                 = "${var.app_project_slug}-${var.app_environment}"
  resource_group_name                  = var.app_project_slug
  location                             = var.arm_location
  version                              = "12.0"
  connection_policy                    = "Default"
  outbound_network_restriction_enabled = false
  public_network_access_enabled        = true

  administrator_login          = local.environment_data_ephemeral ? "sa-${var.app_project_slug}" : null
  administrator_login_password = one(random_password.backend_database_sa_password[*].result)

  azuread_administrator {
    azuread_authentication_only = !local.environment_data_ephemeral
    tenant_id                   = var.arm_tenant_id
    object_id                   = var.arm_client_id
    login_username              = "github-actions-${var.app_project_slug}"
  }

  identity {
    type = "SystemAssigned"
  }

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_mssql_database" "backend_database" {
  name                        = "${var.app_environment}-backend"
  server_id                   = azurerm_mssql_server.backend_database.id
  collation                   = "SQL_Latin1_General_CP1_CI_AS"
  sku_name                    = "GP_S_Gen5_1"
  auto_pause_delay_in_minutes = 60
  min_capacity                = 0.5
  storage_account_type        = "Local"

  dynamic "short_term_retention_policy" {
    for_each = local.backend_database_backups_enabled ? [1] : []
    content {
      retention_days           = 7
      backup_interval_in_hours = 24
    }
  }

  tags = {
    environment = var.app_environment
  }
}

# We need to provide access from the CICD servers, so that they can create the users
resource "azurerm_mssql_firewall_rule" "backend_database_public" {
  name             = "${var.app_environment}-backend"
  server_id        = azurerm_mssql_server.backend_database.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "255.255.255.255"
}
