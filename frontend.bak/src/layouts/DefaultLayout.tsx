import { Box, Flex } from '@chakra-ui/react';
import styles from './DefaultLayout.module.scss';
import AppFooter from "./DefaultLayout/AppFooter";
import AppHeader from "./DefaultLayout/AppHeader";
import AppMenu from "./DefaultLayout/AppMenu";

function DefaultLayout({children}: {children: React.ReactNode}) {
    return (
        <Flex direction="column" className={styles['app-wrapper']}>
            <Box as='header' className={styles['app-header']}><AppHeader /></Box>
            <Flex as='main' className={styles['app-main']} direction="row" >
                <Box as='article' className={styles['app-content']}>{children}</Box>
                <Box as='nav' className={styles['app-menu']}>
                    <AppMenu />
                </Box>
            </Flex>
            <Flex as="footer" className={styles['app-footer']} justifyContent="center" py={4}>
                <AppFooter />
            </Flex>
        </Flex>
    )
}

export default DefaultLayout;
