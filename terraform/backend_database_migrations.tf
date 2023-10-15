resource "azurerm_user_assigned_identity" "backend_database_migrations" {
  name                = "${var.app_project_slug}-${var.app_environment}-database-migrations"
  resource_group_name = var.app_project_slug
  location            = var.arm_location
}

resource "azurerm_role_assignment" "backend_database_migrations" {
  scope                = data.azurerm_container_registry.app.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.backend_database_migrations.principal_id
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
  depends_on = [azurerm_mssql_firewall_rule.backend_database_public]

  database  = azurerm_mssql_database.backend_database.name
  username  = azurerm_user_assigned_identity.backend_database_migrations.name
  object_id = azurerm_user_assigned_identity.backend_database_migrations.client_id

  roles = ["db_owner"]
}

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

    # Command duplicated for local testing
    # @see ../database-tools.sh
    commands = [
      "dotnet", "exec",
      "--runtimeconfig", "./App.Data.Migrations.runtimeconfig.json",
      "--depsfile", "./App.Data.Migrations.deps.json",
      "ef.dll", "--verbose", "database", "update",
      "--context", "App.Data.DatabaseContext",
      "--assembly", "./App.Data.Migrations.dll",
      "--startup-assembly", "./App.Data.Migrations.dll",
    ]

    ports {
      port = 8080
    }

    secure_environment_variables = {
      ConnectionStrings__DefaultConnection = local.backend_app_env.ConnectionStrings__DefaultConnection
    }
  }

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.backend_database_migrations.id
    ]
  }

  image_registry_credential {
    server                    = data.azurerm_container_registry.app.login_server
    user_assigned_identity_id = azurerm_user_assigned_identity.backend_database_migrations.id
  }

  tags = {
    environment = var.app_environment
  }
}
