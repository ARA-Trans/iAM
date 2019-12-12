/**
 * Given an LDAP-formatted string, returns the Common Name (CN) field
 * @param ldap An LDAP-formatted string
 */
export const parseLDAP = (ldap: string) => {
    return ldap.split(',')[0].split('=')[1];
};