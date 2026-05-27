# Octopus CLI v2 Reference

## CLI Installation

The workflow uses the official Octopus CLI GitHub Action:

```yaml
- name: Install Octopus CLI
  uses: OctopusDeploy/install-octopus-cli-action@v3
```

## Verify Installation

```bash
octopus version
```

## Command Reference

### 1. Upload Package

**Old syntax (octo):**
```bash
octo push --package <file> --server <url> --apiKey <key>
```

**New syntax (octopus):**
```bash
octopus package upload \
  --file <file> \
  --server <url> \
  --apiKey <key> \
  --space <space>
```

**Example:**
```bash
octopus package upload \
  --file LegacyLoanProcessor.42.zip \
  --server https://kamloicem.octopus.app/ \
  --apiKey $OCTOPUS_API_KEY \
  --space "Default"
```

### 2. Create Release

**Old syntax (octo):**
```bash
octo create-release --project <name> --version <ver> --server <url>
```

**New syntax (octopus):**
```bash
octopus release create \
  --project <name> \
  --version <ver> \
  --server <url> \
  --apiKey <key> \
  --space <space>
```

**Example:**
```bash
octopus release create \
  --project "meridian - legacy" \
  --version 42 \
  --server https://kamloicem.octopus.app/ \
  --apiKey $OCTOPUS_API_KEY \
  --space "Default"
```

### 3. Deploy Release

**Old syntax (octo):**
```bash
octo deploy-release --project <name> --version <ver> --deployTo <env>
```

**New syntax (octopus):**
```bash
octopus release deploy \
  --project <name> \
  --version <ver> \
  --deployTo <env> \
  --server <url> \
  --apiKey <key> \
  --space <space>
```

**Example:**
```bash
octopus release deploy \
  --project "meridian - legacy" \
  --version 42 \
  --deployTo "Development" \
  --server https://kamloicem.octopus.app/ \
  --apiKey $OCTOPUS_API_KEY \
  --space "Default"
```

## Workflow Usage

### Push Packages Job

```yaml
- name: Push LegacyLoanProcessor to Octopus
  run: |
    octopus package upload \
      --file LegacyLoanProcessor.${{ env.VERSION }}.zip \
      --server ${{ env.OCTOPUS_SERVER_URL }} \
      --apiKey ${{ env.OCTOPUS_API_KEY }} \
      --space "${{ env.OCTOPUS_SPACE }}"
```

### Create Release Job

```yaml
- name: Create release for meridian - legacy
  run: |
    octopus release create \
      --project "meridian - legacy" \
      --version ${{ env.VERSION }} \
      --server ${{ env.OCTOPUS_SERVER_URL }} \
      --apiKey ${{ env.OCTOPUS_API_KEY }} \
      --space "${{ env.OCTOPUS_SPACE }}"
```

### Deploy Release Job

```yaml
- name: Deploy meridian - legacy to Development
  run: |
    octopus release deploy \
      --project "meridian - legacy" \
      --version ${{ env.VERSION }} \
      --deployTo "Development" \
      --server ${{ env.OCTOPUS_SERVER_URL }} \
      --apiKey ${{ env.OCTOPUS_API_KEY }} \
      --space "${{ env.OCTOPUS_SPACE }}"
```

## Common Options

### Authentication
```bash
--server <url>      # Octopus Server URL
--apiKey <key>      # API Key for authentication
--space <name>      # Space name (default: "Default")
```

### Package Options
```bash
--file <path>       # Package file path
--overwrite-mode    # What to do if package exists (FailIfExists, OverwriteExisting, IgnoreIfExists)
```

### Release Options
```bash
--project <name>    # Project name
--version <ver>     # Release version
--channel <name>    # Channel name (optional)
--deployTo <env>    # Environment to deploy to
```

## Migration Guide

If you have existing scripts using `octo`, update them as follows:

| Old Command | New Command |
|------------|-------------|
| `octo push` | `octopus package upload` |
| `octo create-release` | `octopus release create` |
| `octo deploy-release` | `octopus release deploy` |
| `octo list-deployments` | `octopus deployment list` |
| `octo list-releases` | `octopus release list` |

## Troubleshooting

### Command Not Found

If you see `octo: command not found`, you're using old syntax. Update to:
```bash
octopus <command>
```

### Verify CLI Installation

```bash
octopus version
# Should output: Octopus CLI, version X.X.X
```

### Debug Mode

Add `--debug` flag to any command:
```bash
octopus package upload --file package.zip --debug
```

## Additional Resources

- **Official Docs:** https://octopus.com/docs/octopus-rest-api/octopus-cli
- **GitHub Action:** https://github.com/OctopusDeploy/install-octopus-cli-action
- **CLI Repository:** https://github.com/OctopusDeploy/cli

## Notes

- The CLI v2 uses `octopus` as the executable name
- All commands use subcommands (e.g., `octopus release create`)
- Parameters use `--camelCase` format
- The `--space` parameter is now required for Cloud instances
- Project names with spaces must be quoted: `"meridian - legacy"`
