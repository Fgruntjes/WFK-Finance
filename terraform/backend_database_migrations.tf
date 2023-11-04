resource "azurerm_container_group" "backend_database_migrations" {
  name                = "${var.app_project_slug}-${var.app_environment}-database-migrations"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
  os_type             = "Linux"
  restart_policy      = "OnFailure"

  container {
    name   = "app"
    image  = "${data.azurerm_container_registry.app.login_server}/${var.app_environment}/app.data.migrations:${var.app_version}"
    cpu    = "1"
    memory = "2"

    ports {
      port = 8080
    }

    secure_environment_variables = {
      ConnectionStrings__DefaultConnection = local.backend_settings.ConnectionStrings.DefaultConnection
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
