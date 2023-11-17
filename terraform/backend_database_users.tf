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

# Create admin user
resource "azurerm_user_assigned_identity" "backend_database_owner" {
  name                = "${var.app_environment}-backend-database-owner"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "mssql_user" "backend_database_migrations" {
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
  username  = azurerm_user_assigned_identity.backend_database_owner.name
  object_id = azurerm_user_assigned_identity.backend_database_owner.client_id

  roles = ["db_owner"]
}

# Create password user for cicd
output "app_db_connection_string" {
  sensitive = true
  value = join(";", [
    "Server=${azurerm_mssql_server.backend_database.fully_qualified_domain_name}",
    "Database=${azurerm_mssql_database.backend_database.name}",
    //"User Id=${azurerm_mssql_server.backend_database.administrator_login}",
    //"Password=${azurerm_mssql_server.backend_database.administrator_login_password}",
    "Encrypt=True",
  ])
}
