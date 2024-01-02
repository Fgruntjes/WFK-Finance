locals {
  app_frontend_url = var.app_environment == "dev" ? "http://localhost:3000" : "https://${var.app_environment}.${var.app_project_slug}.pages.dev"
}

resource "auth0_client" "frontend" {
  name        = "${var.app_environment}-frontend"
  description = "Frontend for ${var.app_environment}"
  app_type    = "spa"
  callbacks = [
    "${local.app_frontend_url}/auth-callback",
  ]

  oidc_conformant = true
  grant_types = [
    "authorization_code",
    "implicit",
    "refresh_token",
  ]

  jwt_configuration {
    alg = "RS256"
  }
}


resource "auth0_client_credentials" "frontend" {
  client_id = auth0_client.frontend.id

  authentication_method = "none"
}

output "frontend_auth0_client_id" {
  value = auth0_client.frontend.client_id
}

output "app_frontend_url" {
  value = local.app_frontend_url
}
