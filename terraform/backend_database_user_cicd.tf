
resource "random_password" "integration_test_admin_password" {
  count            = var.app_environment == "main" ? 0 : 1
  length           = 16
  special          = true
  override_special = "_%@"
}

resource "mssql_user" "integration_test_admin" {
  count = var.app_environment == "main" ? 0 : 1
  server {
    host = azurerm_mssql_server.backend_database.fully_qualified_domain_name
    azure_login {
      tenant_id     = var.arm_tenant_id
      client_id     = var.arm_client_id
      client_secret = var.arm_client_secret
    }
  }
  depends_on = [azurerm_mssql_firewall_rule.backend_database_public]

  database = azurerm_mssql_database.backend_database.name
  username = "test-admin"
  password = one(random_password.integration_test_admin_password[*].result)

  roles = ["db_owner"]
}
