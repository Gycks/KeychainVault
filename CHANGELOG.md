### v1.0.0

- Initial release.

### v1.0.1

- `LoadXPasswordItem`: account specification removed as the account might not be know at this query
  (See Apple Docs on Updating and deleting keychain items for a reference)
- `DeleteXPasswordItem`: account specification made optional. By default, it should be possible to delete
    an item with just the key (service or server). However, to be more precise, the account along side other
    search options could be passed along too. (See Apple Docs on Updating and deleting keychain items for a reference)

### v1.0.3
- Extend `LoadXPasswordItem` to return not just the secret, but a response object consisiting of the secret
    and some data attributes.