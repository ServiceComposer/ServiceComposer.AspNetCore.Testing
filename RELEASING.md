# Releasing

This is the release process for `ServiceComposer.AspNetCore.Testing`. [ServiceComposer.AspNetCore](https://github.com/ServiceComposer/ServiceComposer.AspNetCore/blob/master/RELEASING.md) uses the same tagging and publishing steps, and also maintains previous majors on release branches.

## Branches and versions

Versions are calculated by [MinVer](https://github.com/adamralph/minver) from git tags. Nothing else sets a version number.

- **`master`** is where development happens, and it builds the current major. Releases are tagged on `master`. `MinVerMinimumMajorMinor`, in `src/ServiceComposer.AspNetCore.Testing/ServiceComposer.AspNetCore.Testing.csproj`, is set to that major.
- **`release-X.Y`** branches are only needed to patch a version when `master` has moved on. The tags for those releases go on the release branch, never on `master`.

The existing `release-1.3` and `release-2.0` branches predate the current CI workflow: `release-1.3` has no `ci.yml` and `release-2.0` publishes to MyGet. Bring the workflow up to date before shipping from either.

MinVer derives untagged versions from the latest tag reachable from the commit. For example, `master` builds `4.2.1-alpha.0.<height>` after `4.2.0`, even if the next release from it is `4.3.0`: the tag decides the version.

To see which release branches exist, and what each branch builds:

```shell
git branch -r --list 'origin/release-*'
dotnet build src/ServiceComposer.AspNetCore.Testing/ServiceComposer.AspNetCore.Testing.csproj -t:MinVer -getProperty:MinVerVersion
```

## Where changes go

1. Every change goes to `master` first, through a pull request.
2. If a change also has to ship from a release branch, cherry-pick it onto a branch created from `release-X.Y` and open a pull request against `release-X.Y`:

   ```shell
   git switch -c backport-<topic>-to-X.Y origin/release-X.Y
   git cherry-pick -x <commit>...
   ```

   `-x` records the original commit in the message. Include any earlier commits the change depends on.
3. Don't backport:
   - Dependabot dependency updates, unless the release needs them.
   - Workflows that only run from the default branch, such as the project board one.

Anything merged to `master` after a release branch is created is **not** in that release unless it's backported.

Dependabot only updates `master`, so release branches keep their own dependency set.

## Creating a release branch

To patch `X.Y` after `master` has moved on, create `release-X.Y` from the `X.Y.0` tag, if it doesn't exist yet:

```shell
git switch -c release-X.Y X.Y.0
git push -u origin release-X.Y
```

## Starting a new major

To start major `X` on `master`:

1. If the previous major can still need patches from its latest commit on `master`, create its release branch first.
2. Open a pull request on `master` that sets `MinVerMinimumMajorMinor` to `X.0`.
3. Tag pre-releases, if any, then `X.0.0`, on `master`, as described below.

## Tagging a release

1. Pick the branch: `master`, unless patching a version `master` has moved past.
2. Make sure everything that should ship is merged into it, including backports, and that its CI build is green. A new vulnerability advisory or SDK update can break a build that used to pass, so if the branch has been quiet, run CI again first:

   ```shell
   gh workflow run ci.yml --ref master
   ```

3. A stable release must not depend on a pre-release. NuGet warns about it when packing (NU5104).
4. Tag the branch and push the tag:

   ```shell
   git switch master        # or release-X.Y
   git pull
   git tag X.Y.Z            # or X.Y.Z-beta.N, X.Y.Z-rc.N
   git push origin X.Y.Z
   ```

   Tags have no `v` prefix. The CI workflow only runs on tags matching `[0-9].[0-9]+.[0-9]` and `[0-9].[0-9]+.[0-9]+-*`, so a stable tag with a two-digit major or patch, such as `4.2.10`, doesn't run it. Update the filters in `.github/workflows/ci.yml` first, on the branch being tagged.
5. The tag build runs the tests and publishes the package:
   - pre-release tags (`X.Y.Z-*`) go to the [Feedz.io pre-releases feed](https://f.feedz.io/servicecomposer/pre-releases/nuget/index.json)
   - stable tags (`X.Y.Z`) go to [NuGet.org](https://www.nuget.org/profiles/ServiceComposer), through [trusted publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing): the workflow file name, `ci.yml`, is part of the nuget.org policy, so renaming the file requires updating the policy.
6. Create the GitHub release from the tag, with notes generated since the previous release:

   ```shell
   gh release create X.Y.Z --verify-tag --generate-notes --notes-start-tag <previous-tag>
   ```

   Add `--prerelease` for pre-release tags, and `--latest=false` when releasing an older line. Call out breaking changes, such as a new minimum .NET version, at the top of the notes.

## Releasing across repositories

`ServiceComposer.AspNetCore` and `ServiceComposer.AspNetCore.Testing` are released independently, and their version numbers aren't aligned. Neither package depends on the other: the tests in `ServiceComposer.AspNetCore` use this package from NuGet.org, and Renovate only proposes patch updates for it there, so that reference has to be updated manually to pick up a new minor or major.
