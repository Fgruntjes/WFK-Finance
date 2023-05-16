import { ChakraProvider } from '@chakra-ui/react';
import { Routes } from '@generouted/react-router';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactQueryDevtools } from '@tanstack/react-query-devtools';
import React from 'react';
import ReactDOM from 'react-dom/client';
import { IntlProvider } from 'react-intl';
import { ToastContainer } from 'react-toastify';
import { RecoilRoot } from 'recoil';

import 'react-toastify/dist/ReactToastify.css';
import './index.scss';

const queryClient = new QueryClient()

ReactDOM.createRoot(document.getElementById('root') as HTMLElement).render(
    <React.StrictMode>
        <ChakraProvider>
            <RecoilRoot>
                <IntlProvider locale="en" defaultLocale="en">
                    <QueryClientProvider client={queryClient}>
                        <Routes />
                        
                        <ToastContainer />
                        {import.meta.env.DEV && <ReactQueryDevtools initialIsOpen={false} />}
                    </QueryClientProvider>
                </IntlProvider>
            </RecoilRoot>
        </ChakraProvider>
    </React.StrictMode>,
)
