resource "azurerm_storage_account" "frontend" {
  name                     = "${replace(var.app_project_slug, "-", "")}${var.app_environment}frontend"
  resource_group_name      = var.app_project_slug
  location                 = var.arm_location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  static_website {
    index_document     = "index.html"
    error_404_document = "404.html"
  }

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_storage_container" "frontend" {
  name                  = "$web"
  storage_account_name  = azurerm_storage_account.frontend.name
  container_access_type = "private"
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

resource "azurerm_storage_blob" "frontend" {
  for_each = local.frontend_files

  name                   = each.value.name
  storage_account_name   = azurerm_storage_account.frontend.name
  storage_container_name = azurerm_storage_container.frontend.name
  type                   = "Block"
  source                 = each.key
  cache_control          = startswith(each.value.name, "_app/immutable") ? "public, max-age=31536000, immutable" : "private, max-age=0, no-cache"
  content_type           = lookup(local.mime_types, each.value.extension, "application/octet-stream")
}

output "frontend_url" {
  value = azurerm_storage_account.frontend.primary_web_host
}
