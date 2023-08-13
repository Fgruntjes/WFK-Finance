import { pino } from 'pino';

export const logger = pino({
  name: 'wfk-finance',
  level: 'debug'
})
