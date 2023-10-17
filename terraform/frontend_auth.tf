resource "auth0_client" "frontend" {
  name        = "${var.app_environment}-backend"
  description = "My Web App Created Through Terraform"
  app_type    = "spa"
  callbacks = local.app_environment_name == "production" ? ["${local.frontend_url}/"] : [
    "http://localhost:3000/",
    "${local.frontend_url}/",
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
