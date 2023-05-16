function AppFooter() {
    const currentYear = new Date().getFullYear();

    return (
        <span>&copy; {currentYear} WFK Finance. All rights reserved.</span>
    )
}

export default AppFooter
