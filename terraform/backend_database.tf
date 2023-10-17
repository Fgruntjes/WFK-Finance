locals {
  app_environment_name = var.app_environment == "main" ? "production" : var.app_environment == "test" ? "test" : "cicd"
}

resource "random_password" "backend_database_admin" {
  length  = 32
  special = true
}

resource "azurerm_mssql_server" "backend_database" {
  name                = "${var.app_project_slug}-${local.app_environment_name}"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
  version             = "12.0"

  administrator_login          = "serveradmin"
  administrator_login_password = random_password.backend_database_admin.result

  azuread_administrator {
    azuread_authentication_only = false
    object_id                   = var.arm_client_id
    login_username              = "cicd-admin"
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

# We need to provide access from the CICD servers somehow
resource "azurerm_mssql_firewall_rule" "backend_database_public" {
  name             = "${var.app_environment}-backend"
  server_id        = azurerm_mssql_server.backend_database.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "255.255.255.255"
}

output "backend_database_admin_connection_string" {
  value = join(";", [
    "Server=${azurerm_mssql_server.backend_database.fully_qualified_domain_name}",
    "Database=${azurerm_mssql_database.backend_database.name}",
    "User ID=${azurerm_mssql_server.backend_database.administrator_login}",
    "Password=${random_password.backend_database_admin.result}",
    "MultipleActiveResultSets=False",
    "Encrypt=True",
  ])
  sensitive = true
}
