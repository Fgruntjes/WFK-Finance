import { notifyError } from '@/components/ErrorElement';
import { BankAccountsApi } from './generated';
import useServiceQuery from './useServiceQuery';

const useBankAccountRefresh = () => useServiceQuery(BankAccountsApi, {
    queryKey: ["BankAccountsApi.bankAccountRefresh"],
    queryFn: (service) => service.bankAccountRefresh(),
    enabled: false,
    onError: notifyError,
});
    
export default useBankAccountRefresh;