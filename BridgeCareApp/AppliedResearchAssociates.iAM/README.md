# AppliedResearchAssociates.iAM

## Comments

- TODO
  - why does Box exist? e.g. no need to "box" properties that don't participate in validation. the purpose of box (as used here) is to provide a persistent target object with which validation results can be associated.
  - what is the overall validation pattern?
    - data subdomains vs behavior subdomains. input/mutable/entity data subdomains vs output/immutable/value data subdomains.
    - the general pattern here is to use (a) validation results for input/entities and (b) validation exceptions for behavior and output generation.

> TODO

- Cross-module comments.
