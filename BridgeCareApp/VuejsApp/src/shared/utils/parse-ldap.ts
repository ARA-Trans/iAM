/**
 * Given an LDAP-formatted string, returns a list of all Common Name (CN) fields
 * @param ldap An LDAP-formatted string
 */
export const parseLDAP = (ldap: string) => {
    return ldap.split('^').map(segment => segment.split(',')[0].split('=')[1]);
};

/**
 * Checks an LDAP-formatted string for a specific Common Name (CN) field
 * @param ldap An LDAP-formatted string
 * @param cn The CN to look for
 */
export const checkLDAP = (ldap: string, cn: string) => {
    return parseLDAP(ldap).indexOf(cn) >= 0;
};