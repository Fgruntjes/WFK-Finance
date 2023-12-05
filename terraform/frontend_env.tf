resource "local_file" "dev_env_frontend" {
  content = join("\n", [
    "APP_API_URI=http://localhost:8080",
    "AUTH0_AUDIENCE=${auth0_resource_server.backend.identifier}",
    "AUTH0_DOMAIN=${var.auth0_domain}",
    "AUTH0_CLIENT_ID=${auth0_client.frontend.client_id}",
    "SENTRY_DSN=${sentry_key.frontend.dsn_public}",
  ])
  filename = "../frontend/.env.local"
}
