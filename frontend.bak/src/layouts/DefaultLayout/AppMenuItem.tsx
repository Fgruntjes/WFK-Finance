import { Link } from '@chakra-ui/react';
import classnames from 'classnames';
import { ReactNode } from "react";
import { NavLink, To } from "react-router-dom";
import styles from './AppMenuItem.module.scss';

type AppMenuItemProps = {
    to: To;
    icon?: ReactNode;
    children: ReactNode;
}

function AppMenuItem(props: AppMenuItemProps) {
    function classNameResolver({ isActive }: { isActive: boolean, isPending: boolean }) {
        return classnames({
            [styles['nav-link']]: true,
            [styles['active']]: isActive,
        });
    }
    
    function AppNavLink({ children }: { children: ReactNode }) {
        return (
            <NavLink to={props.to} className={classNameResolver}>{children}</NavLink>
        );
    }

    return (
        <Link as={AppNavLink}>
            {props.icon}
            <span>{props.children}</span>
        </Link>
    )
}

export default AppMenuItem
