resource "azurerm_container_app" "backend_app" {
  name                         = "v${var.app_environment}-backend-app"
  container_app_environment_id = azurerm_container_app_environment.backend.id
  resource_group_name          = var.app_project_slug
  revision_mode                = "Single"

  tags = {
    environment = var.app_environment
  }

  depends_on = [azurerm_role_assignment.container_registry_pull]

  ingress {
    external_enabled = true
    target_port      = 8080
    traffic_weight {
      # Keep @see https://github.com/hashicorp/terraform-provider-azurerm/issues/21022
      latest_revision = true
      percentage      = 100
    }
  }

  identity {
    type = "UserAssigned"
    identity_ids = [
      azurerm_user_assigned_identity.backend_database_read_write.id,
      azurerm_user_assigned_identity.keyvault.id,
      azurerm_user_assigned_identity.container_registry_pull.id,
      azurerm_user_assigned_identity.service_bus_send.id,
    ]
  }

  registry {
    server   = data.azurerm_container_registry.app.login_server
    identity = azurerm_user_assigned_identity.container_registry_pull.id
  }

  secret {
    name = "settings"
    value = jsonencode(merge(local.backend_settings, {
      ConnectionStrings = {
        Database   = local.connection_strings.database.readwrite
        ServiceBus = local.connection_strings.service_bus.prod
      }
    }))
  }

  template {
    min_replicas = 0
    max_replicas = 10

    container {
      name  = "app"
      image = "${data.azurerm_container_registry.app.login_server}/app.backend:${var.app_version}"
      command = [
        "bash",
        "-c",
        "echo \"$${AppSettings}\" > appsettings.local.json && dotnet App.Backend.dll",
      ]
      cpu    = 0.5
      memory = "1Gi"
      env {
        name        = "AppSettings"
        secret_name = "settings"
      }
      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }
      liveness_probe {
        path      = "/.health/live"
        port      = 8080
        transport = "HTTP"
      }
      readiness_probe {
        path      = "/.health/ready"
        port      = 8080
        transport = "HTTP"
      }
    }

    http_scale_rule {
      name                = "concurrency"
      concurrent_requests = 10
    }
  }
}

locals {
  app_api_url = "https://${azurerm_container_app.backend_app.ingress[0].fqdn}"
}

output "app_api_url" {
  value = local.app_api_url
}
