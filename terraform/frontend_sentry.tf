resource "sentry_project" "frontend" {
  organization = var.sentry_organisation

  name  = "${var.app_project_slug} ${var.app_environment} Frontend"
  slug  = "${var.app_project_slug}-${var.app_environment}-frontend"
  teams = [sentry_team.default.slug]

  platform    = "javascript-sveltekit"
  resolve_age = 720
}

resource "sentry_key" "frontend" {
  organization = var.sentry_organisation

  project = sentry_project.frontend.slug
  name    = "Key"
}

output "frontend_sentry_dsn" {
  value = sentry_key.frontend.dsn_public
}
