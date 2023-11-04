resource "azurerm_storage_account" "frontend_app" {
  name                     = "${replace(var.app_project_slug, "-", "")}${var.app_environment}frontend"
  resource_group_name      = var.app_project_slug
  location                 = var.arm_location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  static_website {
    index_document     = "index.html"
    error_404_document = "index.html"
  }

  tags = {
    environment = var.app_environment
  }
}

locals {
  frontend_files = {
    for file in fileset(path.module, "../frontend/build/**") : file => {
      name      = replace(file, "../frontend/build/", "")
      extension = element(split(".", file), length(split(".", file)) - 1)
    }
  }
  mime_types = {
    "js"   = "application/javascript",
    "json" = "application/json",
    "html" = "text/html"
    "png"  = "image/png"
    "jpg"  = "image/jpg"
    "svg"  = "image/svg"
    "css"  = "text/css"
  }
}

resource "local_file" "dev_env_frontend" {
  content = join("\n", [
    "APP_API_URI=http://localhost:8080",
    "AUTH0_AUDIENCE=${auth0_resource_server.backend.identifier}",
    "AUTH0_DOMAIN=${var.auth0_domain}",
    "AUTH0_CLIENT_ID=${auth0_client.frontend.client_id}",
  ])
  filename = "../frontend/.env.local"
}

resource "azurerm_storage_blob" "frontend_app" {
  for_each   = local.frontend_files
  depends_on = []

  name                   = each.value.name
  storage_account_name   = azurerm_storage_account.frontend_app.name
  storage_container_name = "$web"
  type                   = "Block"
  source_content = each.value.extension == "js" || each.value.extension == "html" ? replace(
    replace(
      replace(
        replace(
          file(each.key),
          "__APP_API_URI__",
          jsonencode(local.backend_url)
        ),
        "__AUTH0_AUDIENCE__",
        jsonencode(auth0_resource_server.backend.identifier)
      ),
      "__AUTH0_DOMAIN__",
      jsonencode(var.auth0_domain)
    ),
    "__AUTH0_CLIENT_ID__",
    jsonencode(auth0_client.frontend.client_id)
  ) : null
  source        = each.value.extension != "js" && each.value.extension != "html" ? each.key : null
  cache_control = startswith(each.value.name, "_app/immutable") ? "public, max-age=31536000, immutable" : "private, max-age=0, no-cache"
  content_type  = lookup(local.mime_types, each.value.extension, "application/octet-stream")
}
