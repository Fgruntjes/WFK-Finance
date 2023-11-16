resource "azurerm_user_assigned_identity" "backend_database_read_write" {
  name                = "${var.app_environment}-backend-database-read-write"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "time_sleep" "database_create" {
  depends_on = [
    azurerm_mssql_firewall_rule.backend_database_public,
    azurerm_mssql_server.backend_database,
    azurerm_mssql_database.backend_database,
  ]

  create_duration = "15s"
}

resource "mssql_user" "read_write" {
  server {
    host = azurerm_mssql_server.backend_database.fully_qualified_domain_name
    azure_login {
      client_id     = var.arm_client_id
      tenant_id     = var.arm_tenant_id
      client_secret = var.arm_client_secret
    }
  }
  depends_on = [
    time_sleep.database_create,
  ]
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
      client_id     = var.arm_client_id
      tenant_id     = var.arm_tenant_id
      client_secret = var.arm_client_secret
    }
  }
  depends_on = [
    time_sleep.database_create,
  ]

  database  = azurerm_mssql_database.backend_database.name
  username  = azurerm_user_assigned_identity.backend_database_owner.name
  object_id = azurerm_user_assigned_identity.backend_database_owner.client_id

  roles = ["db_owner"]
}
