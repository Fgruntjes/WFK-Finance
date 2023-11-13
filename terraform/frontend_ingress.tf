resource "azurerm_cdn_frontdoor_profile" "frontend_ingress" {
  name                = "${var.app_environment}-frontend"
  resource_group_name = var.app_project_slug
  sku_name            = "Standard_AzureFrontDoor"

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_cdn_frontdoor_endpoint" "frontend_ingress" {
  name                     = "${var.app_environment}-frontend"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.frontend_ingress.id

  tags = {
    environment = var.app_environment
  }
}

resource "azurerm_cdn_frontdoor_origin_group" "frontend_ingress" {
  name                     = "${var.app_environment}-frontend"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.frontend_ingress.id
  session_affinity_enabled = false

  health_probe {
    path                = "/"
    request_type        = "HEAD"
    protocol            = "Http"
    interval_in_seconds = 100
  }

  load_balancing {
    additional_latency_in_milliseconds = 50
    sample_size                        = 4
    successful_samples_required        = 3
  }
}

resource "azurerm_cdn_frontdoor_origin" "frontend_ingress" {
  name                           = "${var.app_environment}-frontend"
  cdn_frontdoor_origin_group_id  = azurerm_cdn_frontdoor_origin_group.frontend_ingress.id
  enabled                        = true
  host_name                      = azurerm_storage_account.frontend_app.primary_web_host
  origin_host_header             = azurerm_storage_account.frontend_app.primary_web_host
  certificate_name_check_enabled = true
  weight                         = 100
  priority                       = 1
  http_port                      = 80
  https_port                     = 443
}

resource "azurerm_cdn_frontdoor_route" "frontend_ingress" {
  name                          = "${var.app_environment}-frontend"
  cdn_frontdoor_endpoint_id     = azurerm_cdn_frontdoor_endpoint.frontend_ingress.id
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.frontend_ingress.id
  cdn_frontdoor_origin_ids      = [azurerm_cdn_frontdoor_origin.frontend_ingress.id]
  cdn_frontdoor_rule_set_ids    = []
  enabled                       = true

  forwarding_protocol    = "HttpsOnly"
  https_redirect_enabled = true
  patterns_to_match      = ["/*"]
  supported_protocols    = ["Http", "Https"]
  link_to_default_domain = true

  cache {
    query_string_caching_behavior = "IgnoreQueryString"
    compression_enabled           = true
    content_types_to_compress = [
      "application/eot",
      "application/font",
      "application/font-sfnt",
      "application/javascript",
      "application/json",
      "application/opentype",
      "application/otf",
      "application/pkcs7-mime",
      "application/truetype",
      "application/ttf",
      "application/vnd.ms-fontobject",
      "application/xhtml+xml",
      "application/xml",
      "application/xml+rss",
      "application/x-font-opentype",
      "application/x-font-truetype",
      "application/x-font-ttf",
      "application/x-httpd-cgi",
      "application/x-javascript",
      "application/x-mpegurl",
      "application/x-opentype",
      "application/x-otf",
      "application/x-perl",
      "application/x-ttf",
      "font/eot",
      "font/ttf",
      "font/otf",
      "font/opentype",
      "image/svg+xml",
      "text/css",
      "text/csv",
      "text/html",
      "text/javascript",
      "text/js",
      "text/plain",
      "text/richtext",
      "text/tab-separated-values",
      "text/xml",
      "text/x-script",
      "text/x-component",
      "text/x-java-source"
    ]
  }
}

locals {
  frontend_url = "https://${azurerm_cdn_frontdoor_endpoint.frontend_ingress.host_name}"
}

output "frontend_url" {
  value = local.frontend_url
}
