import configuration from './configuration';
import { BankAccountListApi } from './generated';

export const BankAccountsService = new BankAccountListApi(configuration);
