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

## Authentication

Octopus CLI v2 expects authentication via environment variables:

```bash
export OCTOPUS_URL="https://kamloicem.octopus.app/"
export OCTOPUS_API_KEY="your-api-key"
```

Once set, all commands automatically use these credentials.

## Command Reference

### 1. Upload Package

**Old syntax (octo):**
```bash
octo push --package <file> --server <url> --apiKey <key>
```

**New syntax (octopus v2 - with environment variables):**
```bash
# Set authentication once
export OCTOPUS_URL="https://kamloicem.octopus.app/"
export OCTOPUS_API_KEY="your-api-key"

# Then use commands without auth flags
octopus package upload \
  --file <file> \
  --space <space>
```

**Example:**
```bash
export OCTOPUS_URL="https://kamloicem.octopus.app/"
export OCTOPUS_API_KEY=$OCTOPUS_API_KEY

octopus package upload \
  --file LegacyLoanProcessor.42.zip \
  --space "Default"
```

### 2. Create Release

**Old syntax (octo):**
```bash
octo create-release --project <name> --version <ver> --server <url>
```

**New syntax (octopus v2 - with environment variables):**
```bash
# Set authentication once
export OCTOPUS_URL="https://kamloicem.octopus.app/"
export OCTOPUS_API_KEY="your-api-key"

# Then use commands without auth flags
octopus release create \
  --project <name> \
  --version <ver> \
  --space <space>
```

**Example:**
```bash
export OCTOPUS_URL="https://kamloicem.octopus.app/"
export OCTOPUS_API_KEY=$OCTOPUS_API_KEY

octopus release create \
  --project "meridian - legacy" \
  --version 42 \
  --space "Default"
```

### 3. Deploy Release

**Old syntax (octo):**
```bash
octo deploy-release --project <name> --version <ver> --deployTo <env>
```

**New syntax (octopus v2 - with environment variables):**
```bash
# Set authentication once
export OCTOPUS_URL="https://kamloicem.octopus.app/"
export OCTOPUS_API_KEY="your-api-key"

# Then use commands without auth flags
octopus release deploy \
  --project <name> \
  --version <ver> \
  --deployTo <env> \
  --space <space>
```

**Example:**
```bash
export OCTOPUS_URL="https://kamloicem.octopus.app/"
export OCTOPUS_API_KEY=$OCTOPUS_API_KEY

octopus release deploy \
  --project "meridian - legacy" \
  --version 42 \
  --deployTo "Development" \
  --space "Default"
```

## Workflow Usage

### Job-Level Environment Variables

```yaml
jobs:
  push-to-octopus:
    name: Push Packages to Octopus Deploy
    runs-on: ubuntu-latest
    env:
      OCTOPUS_URL: ${{ secrets.OCTOPUS_SERVER_URL }}
      OCTOPUS_API_KEY: ${{ secrets.OCTOPUS_API_KEY }}
      OCTOPUS_SPACE: Default
```

### Verify Authentication

```yaml
- name: Verify Authentication
  run: octopus space list
```

### Push Packages Job

```yaml
- name: Push LegacyLoanProcessor to Octopus
  run: |
    octopus package upload \
      --file LegacyLoanProcessor.${{ github.run_number }}.zip \
      --space "${{ env.OCTOPUS_SPACE }}"
```

### Create Release Job

```yaml
- name: Create release for meridian - legacy
  run: |
    octopus release create \
      --project "meridian - legacy" \
      --version ${{ github.run_number }} \
      --space "${{ env.OCTOPUS_SPACE }}"
```

### Deploy Release Job

```yaml
- name: Deploy meridian - legacy to Development
  run: |
    octopus release deploy \
      --project "meridian - legacy" \
      --version ${{ github.run_number }} \
      --deployTo "Development" \
      --space "${{ env.OCTOPUS_SPACE }}"
```

## Common Options

### Authentication (via Environment Variables)
```bash
OCTOPUS_URL         # Octopus Server URL
OCTOPUS_API_KEY     # API Key for authentication
```

### Command Options
```bash
--space <name>      # Space name (required for Cloud)
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
