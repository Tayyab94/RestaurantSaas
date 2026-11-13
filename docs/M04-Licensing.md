# M04 Licensing Core: notes and checks

Status: **code written, not compiled or run** (no .NET SDK was available). Build first and fix errors.
No schema change, so **no migration**: all license tables already existed.

## What it does

| Area | Behavior |
|---|---|
| Keys | `XXXXX-XXXXX-XXXXX-XXXXX` (16 random + 4 checksum, Crockford alphabet). Only an HMAC hash is stored (`License:KeyPepper`). The plain key is shown once at creation. |
| License file | `license.lic` = JSON with payload and ECDSA P-256 signature, in `%LocalAppData%\RestoPOS` (override `License:FilePath`). |
| Desktop and on-premise | `/activate` before first-run setup: key + holder name, or 14-day trial (name, phone, optional email). The License Server returns the signed file for this PC. |
| Cloud | The owner enters a key on `/license` (permission `License.Activate`). A `Subscription` is created or extended. The sign-up trial keeps working until then. |
| Start-up check | Signature, PC fingerprint (3 of 4 hashed parts), sequence against the last `LicenseRecord`, record hash chain, clock mark. Writes `LicenseCheckLog`. Cached 5 minutes. |
| States | NotActivated, Trial, Active, Expiring (14 days), Grace (7 days, paid only), Expired, Tampered, ClockError, Mismatch, Revoked, NotRequired. |
| Selling guard | `[RequireActiveLicense]` stops a request when selling is blocked. SignalR `PosHub` refuses the connection. M09 and M11 must add the attribute. |
| System lock | A non-cloud PC that is NotActivated refuses every non-anonymous request (`LicenseBehavior`); screens redirect to `/activate` (`LicenseLock`). |
| License Server | Generate keys (SuperAdmin `/admin/license-keys`), activate, start trial (one per PC, phone, email), deactivate, `LicenseEvent`, `LicenseAttempt` (10 wrong keys per IP in 15 min locks). |
| Deactivate | Permission `License.Deactivate` (Owner by default, can be given to other roles). The signed file is the proof. The server frees the activation, then the file is deleted. Same key can be activated again (end date unchanged). A removed trial cannot be restarted. Cloud: not supported. |
| Change key | Entering another key on `/license` replaces the license. A paid old key is freed on the server automatically; a trial is marked Superseded. |
| Free PCs | SuperAdmin button per key: frees every PC of the key (lost PC). |
| Hourly job (Web) | Cloud: marks ended subscriptions and licenses Expired. On-premise: checks the file again. |

## Settings by deployment

* **Cloud web (License Server):** `License:ServerMode=Embedded`, `License:ServerEnabled=true`, `License:KeyPepper`, `License:SigningPrivateKeyPem`. Default in `appsettings.json`.
* **On-premise web:** `License:ServerMode=Remote`, `License:ServerEnabled=false`, `License:ServerUrl`, `License:TrustedKeys:k1`.
* **Standalone desktop:** release builds read preferences `license_server_url` and `license_public_key` (the installer writes them). Debug builds are their own License Server with a dev key (`%LocalAppData%\RestoPOS\dev-license-signing.pem`).
* Production secrets (`License__KeyPepper`, `License__SigningPrivateKeyPem`) only from environment variables.

## Permissions

`License.View`, `License.Activate`, `License.Deactivate` (tenant), `License.Generate` (platform: SuperAdmin only).

## Not in M04 (by plan)

Offline activation file, transfer, revoke screen, renewal reminders, OS-protected secret, online re-check, repair after restore (M17, M37); plans and feature switches (M34); SuperAdmin license portal (M35).
Known limits: the record chain key comes from the PC fingerprint (stops careless edits, not a determined attacker; M17 hardens it). After deactivation an old copy of `license.lic` may still run offline until the online re-check exists (M17).

## Manual checks (after the first successful build)

1. Debug desktop, no license: sign-in goes to `/activate`; Register also goes there.
2. Start the free trial: sidebar card shows 14 days; banner shows trial text. A second trial on the same PC, phone or email is refused.
3. SuperAdmin: `/admin/license-keys`, make 1 key for 12 months, copy it. A batch of 600 is refused (max 500).
4. Owner: enter the key on `/license`: license Active; the trial license is Superseded on the server.
5. Enter a second key on `/license` ("Change license key"): the first key shows Used 0/1 in the keys list.
6. Wrong key 10 times: `too_many_attempts`.
7. Edit `license.lic` by one character: next start shows Tampered. Copy it to another PC: Mismatch. Put an older file back: Mismatch.
8. Move the Windows clock back a day: ClockError. Forward 15 days on a trial: Expired.
9. Owner deactivates (type DEACTIVATE): `license.lic` is gone, every screen goes to `/activate`, other requests give 402. Enter the same key again: works, same end date.
10. A user without `License.Deactivate` does not see the Deactivate card; the server refuses the call.
11. Cloud: sign up, enter a key on `/license`: Subscription Active, tenant leaves Trial. Set `EndDate` in the past: Grace for 7 days, then Expired.
