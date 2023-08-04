import * as pino from "pino";

const logger = pino({
  name: 'wfk-finance',
  level: 'debug'
});

export default logger;
