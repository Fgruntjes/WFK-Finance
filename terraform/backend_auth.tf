resource "auth0_resource_server" "backend" {
  name        = "${var.app_environment}-backend"
  identifier  = "https://${var.app_environment}-backend"
  signing_alg = "RS256"

  allow_offline_access                            = true
  token_lifetime                                  = 8600
  skip_consent_for_verifiable_first_party_clients = true
}
