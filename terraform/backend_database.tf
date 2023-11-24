resource "azurerm_mssql_server" "backend_database" {
  name                                 = "${var.app_project_slug}-${var.app_environment}"
  resource_group_name                  = var.app_project_slug
  location                             = var.arm_location
  version                              = "12.0"
  connection_policy                    = "Default"
  outbound_network_restriction_enabled = false
  public_network_access_enabled        = true

  azuread_administrator {
    azuread_authentication_only = var.app_environment == "main"
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
  name         = "${var.app_environment}-backend"
  server_id    = azurerm_mssql_server.backend_database.id
  collation    = "SQL_Latin1_General_CP1_CI_AS"
  license_type = "LicenseIncluded"
  sku_name     = "S0"
  # auto_pause_delay_in_minutes = 60
  storage_account_type = "Local"

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
