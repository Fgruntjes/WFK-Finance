resource "random_password" "database" {
  length  = 16
  special = true
}

resource "azurerm_mssql_server" "database" {
  name                = "${var.app_project_slug}-${var.app_environment}"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
  version             = "12.0"

  administrator_login          = "app"
  administrator_login_password = random_password.database.result

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_storage_account" "database_backend" {
  name                     = "${replace(var.app_project_slug, "-", "")}${var.app_environment}database"
  resource_group_name      = var.app_project_slug
  location                 = var.arm_location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_mssql_database" "backend" {
  name                        = "backend"
  server_id                   = azurerm_mssql_server.database.id
  collation                   = "SQL_Latin1_General_CP1_CI_AS"
  license_type                = "LicenseIncluded"
  sku_name                    = "S0"
  auto_pause_delay_in_minutes = 60
  storage_account_type        = "Local"

  tags = {
    environment = var.app_environment
  }
}
