locals {
  environment_data_ephemeral = var.app_environment != "main" && var.app_environment != "test"
  backend_settings = {
    App = {
      Environment = var.app_environment,
      Version     = var.app_version,
      FrontendUrl = local.app_frontend_url,
    }
    DataProtection = {
      Enabled           = true,
      KeyVaultUri       = azurerm_key_vault.app.vault_uri,
      KeyName           = azurerm_key_vault_key.backend_data_protection.name,
      StorageAccountUri = azurerm_storage_account.backend.primary_blob_endpoint,
      StorageContainer  = azurerm_storage_container.backend_data_protection.name,
      KeyBlobName       = azurerm_storage_blob.backend_data_protection.name,
    }
    Auth = {
      Domain   = var.auth0_domain,
      Audience = auth0_resource_server.backend.identifier,
    }
    Nordigen = {
      SecretId  = var.nordigen_secret_id,
      SecretKey = var.nordigen_secret_key,
    }
    ConnectionStrings = {
      Database = "",
    }
    Sentry = {
      Dsn = sentry_key.backend.dsn_public,
    }
  }
  backend_settings_string = jsonencode(merge(local.backend_settings, {
    ConnectionStrings = {
      Database = join(";", [
        "Server=localhost,1433",
        "Database=development",
        "User Id=sa",
        "Password=myLeet123Password!",
        "Encrypt=False",
      ]),
    }
  }))
}

resource "local_file" "dev_env_tests" {
  content  = local.backend_settings_string
  filename = "../appsettings.local.json"
}

output "app_settings_json" {
  sensitive = true
  value = jsonencode(merge(local.backend_settings, {
    ConnectionStrings = {
      Database = local.environment_data_ephemeral ? join(";", [
        "Server=${azurerm_mssql_server.backend_database.fully_qualified_domain_name}",
        "Database=${azurerm_mssql_database.backend_database.name}",
        try("User Id=${one(mssql_user.integration_test_admin[*].username)}", "User Id="),
        try("Password=${one(mssql_user.integration_test_admin[*].password)}", "Password="),
        "Encrypt=True",
      ]) : ""
    }
  }))
}
