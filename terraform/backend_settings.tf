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
      DefaultConnection = "",
    }
  }
}
