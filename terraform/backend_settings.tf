locals {
  environment_data_ephemeral = var.app_environment != "main" && var.app_environment != "test"
  backend_settings = {
    App = {
      Environment = var.app_environment,
      Version     = var.app_version,
      FrontendUrl = local.app_frontend_url
      KeyVaultUri = azurerm_key_vault.app.vault_uri,
      KeyName     = azurerm_key_vault_key.backend_data_protection.name,
    }
    Auth0 = {
      Domain   = var.auth0_domain,
      Audience = auth0_resource_server.backend.identifier,
    }
    Nordigen = {
      SecretId  = var.nordigen_secret_id,
      SecretKey = var.nordigen_secret_key,
    }
    ConnectionStrings = {
      DefaultConnection = "",
    }
    Sentry = {
      Dsn = sentry_key.backend.dsn_public,
    }
  }
  backend_settings_string = jsonencode(merge(local.backend_settings, {
    ConnectionStrings = {
      DefaultConnection = join(";", [
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
  for_each = toset([
    "../App.Test/appsettings.local.json",
    "../App.Test/bin/Debug/net7.0/appsettings.local.json",
    "../App.Backend/appsettings.local.json",
    "../App.Backend/bin/Debug/net7.0/appsettings.local.json",
  ])
  content  = local.backend_settings_string
  filename = each.value
}

output "app_settings_json" {
  sensitive = true
  value = jsonencode(merge(local.backend_settings, {
    ConnectionStrings = {
      DefaultConnection = local.environment_data_ephemeral ? join(";", [
        "Server=${azurerm_mssql_server.backend_database.fully_qualified_domain_name}",
        "Database=${azurerm_mssql_database.backend_database.name}",
        try("User Id=${one(mssql_user.integration_test_admin[*].username)}", "User Id="),
        try("Password=${one(mssql_user.integration_test_admin[*].password)}", "Password="),
        "Encrypt=True",
      ]) : ""
    }
  }))
}
