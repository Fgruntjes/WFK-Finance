resource "azurerm_container_group" "backend_database_migrations" {
  name                = "${var.app_project_slug}-${var.app_environment}-database-migrations"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
  os_type             = "Linux"
  restart_policy      = "OnFailure"

  depends_on = [azurerm_role_assignment.container_registry_pull]

  container {
    name   = "app"
    image  = "${data.azurerm_container_registry.app.login_server}/app.data.migrations:${var.app_version}"
    cpu    = "1"
    memory = "2"

    ports {
      port = 8080
    }

    secure_environment_variables = {
      Sentry__Dsn = local.backend_settings.Sentry.Dsn,
      ConnectionStrings__Database = join(";", [
        "Server=${azurerm_mssql_server.backend_database.fully_qualified_domain_name}",
        "Database=${azurerm_mssql_database.backend_database.name}",
        "Authentication=Active Directory Managed Identity",
        "User Id=${azurerm_user_assigned_identity.backend_database_owner.client_id}",
        "Encrypt=True",
      ])
    }
  }

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.backend_database_owner.id,
      azurerm_user_assigned_identity.container_registry_pull.id,
    ]
  }

  image_registry_credential {
    server                    = data.azurerm_container_registry.app.login_server
    user_assigned_identity_id = azurerm_user_assigned_identity.container_registry_pull.id
  }

  tags = {
    environment = var.app_environment
  }
}
