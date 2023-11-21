resource "azurerm_user_assigned_identity" "backend_database_read_write" {
  name                = "${var.app_environment}-backend-database-read-write"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "mssql_user" "read_write" {
  server {
    host = azurerm_mssql_server.backend_database.fully_qualified_domain_name
    azure_login {
      tenant_id     = var.arm_tenant_id
      client_id     = var.arm_client_id
      client_secret = var.arm_client_secret
    }
  }
  depends_on = [azurerm_mssql_firewall_rule.backend_database_public]

  database  = azurerm_mssql_database.backend_database.name
  username  = azurerm_user_assigned_identity.backend_database_read_write.name
  object_id = azurerm_user_assigned_identity.backend_database_read_write.client_id

  roles = ["db_datareader", "db_datawriter"]
}
