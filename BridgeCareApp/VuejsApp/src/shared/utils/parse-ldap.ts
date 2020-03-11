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

/**
 * Checks whether a provided regex is matched in any of the Common Name fields of an LDAP-formatted string
 * @param ldap An LDAP-formatted string
 * @param regex A regexp
 */
export const regexCheckLDAP = (ldap: string, regex: RegExp) => {
    return parseLDAP(ldap).map(s => !!s.match(regex)).reduce((a,b) => a || b);
};