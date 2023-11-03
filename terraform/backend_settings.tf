locals {
  backend_settings = {
    Auth0 = {
      Domain   = var.auth0_domain,
      Audience = auth0_resource_server.backend.identifier,
    }
    Nordigen = {
      SecretId  = var.nordigen_secret_id,
      SecretKey = var.nordigen_secret_key,
    }
    ConnectionStrings = {
      DefaultConnection = join(";", [
        "Server=${azurerm_mssql_server.backend_database.fully_qualified_domain_name}",
        "Database=${azurerm_mssql_database.backend_database.name}",
        "Authentication=Active Directory Default",
        "Encrypt=True",
      ]),
    }
  }
  backend_settings_local = merge(local.backend_settings, {
    ConnectionStrings = {
      DefaultConnection = join(";", [
        "Server=localhost,1433",
        "Database=development",
        "User Id=sa",
        "Password=myLeet123Password!",
        "Encrypt=False",
      ]),
    }
  })
}
