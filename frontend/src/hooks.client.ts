import * as Sentry from '@sentry/sveltekit';

Sentry.init({
	dsn: import.meta.env.SENTRY_DSN
});
