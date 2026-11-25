# M05 Business Settings

Status: **written, not yet built or tested.** Build first and fix compile errors.
**No schema change, so no migration.** The only new database content is seed data (modules and business-type templates), written at start-up by `ModuleCatalogSeeder`.

## Screens (Administration menu)

| Screen | Route | Permission | FR |
|---|---|---|---|
| Business profile and logo | `/settings/business` | Settings.View / Settings.Manage | FR-M05-01 |
| Branches | `/settings/branches` | Branch.View / Branch.Manage | FR-M05-01 |
| Tax and billing (currency, tax rates, service charge, rounding, test bill) | `/settings/tax` | Settings.View / Settings.Manage | FR-M05-02 |
| Payment methods | `/settings/payment-methods` | Settings.View / Settings.Manage | FR-M05-03 |
| Receipt design | `/settings/receipt` | Settings.View / Settings.Manage | FR-M05-04 |
| Document numbers | `/settings/numbering` | Settings.View / Settings.Manage | FR-M05-05 |
| Modules and business type | `/settings/modules` | Settings.View / Settings.Manage | FR-M05-06 |
| Onboarding auto-tick | checklist on the dashboard | | FR-M05-07 |

Endpoints are under `/api/v1/settings/...` (see `SettingsEndpoints.cs`).

## Where data is stored

| Data | Place |
|---|---|
| Name, owner, phone, email, business type, time zone, language, currency | `Tenant` |
| Address, tax number, logo (data URI, up to 200 KB) | `Setting`, group Business |
| Branch details | `Branch`; opening hours in `Setting` with the branch id |
| Service charge, rounding | `Setting`, group Billing |
| Tax rates, payment methods, receipt templates | `TaxRate`, `PaymentMethod`, `ReceiptTemplate` |
| Document numbers | `NumberSequence` (one row per branch and document, created when first needed) |
| Module switches | `TenantFeature`; the catalog is in `Feature`, `BusinessTypeTemplate`, `TemplateFeature` |

Currencies are a fixed list in code (`CurrencyCatalog`); the `Currency` table is not used yet.

## For the next modules

* **M11 Billing** must use `BillCalculator` (service charge, tax, rounding) and the `Billing` settings read by `SettingStore.GetBillingAsync`.
* **M09 / M11 / M15** get the next number from `NumberSequenceService.NextAsync(branchId, DocumentKinds.Order | Invoice | Payment | Ticket | Shift)`. It changes the row but does not save; save it together with the new document. If two requests collide, the second gets a concurrency error and must retry.
* **A new optional module** is added to `ModuleCatalog` (key `module.xxx`), then its menu item gets `Feature: "module.xxx"` in `NavMenu.razor`.
* **Receipt printing (M10)** reads `ReceiptTemplate` by document type and language.

## Rules that are assumptions (confirm against the SRS)

* Service charge is a percent of the items total, and is not taxed.
* Tax is worked out on the items total. Only taxes marked "Add to every new bill" are used. "Applies to" is stored but not used before M06.
* A fixed tax is a flat amount per bill.
* Rounding is applied last, to the total.
* A module with no saved switch follows the business type: Restaurant (all on), Cafe (inventory), Bakery (inventory, delivery), Fast food (kitchen display, delivery), Retail (inventory). With no template it is on.
* Applying a business-type template changes the business type and replaces all module switches.
* "Reset to defaults" on the Modules screen applies the current business type again. The audit log keeps the old business type and old switches (`template.applied`, `modules.updated`).
* A payment method is switched off, never deleted, and the last active one cannot be switched off.
* One receipt template per document type (Invoice or Kitchen ticket) and language.
* Number reset dates use the business time zone.

## Done when (Development Flow)

"Tax and service charge appear correctly on a test bill; switched-off modules are hidden." Both can be checked on screen (see below).

## Manual checks (after the first successful build, signed in as Owner)

1. Business profile: fill the address, save. The onboarding step "Business information" is ticked.
2. Logo: a PNG of about 100 KB shows. A 300 KB picture and a renamed text file are refused. Remove works.
3. Branches: edit the main branch (address, phone, opening hours): the step "Branch details" is ticked. Add a second branch; a repeated code is refused.
4. Tax and billing: add GST 16% (percent, "Add to every new bill"). Turn on service charge 5%, rounding "To the nearest" multiple of 10. Test bill for 1000: items 1000, service 50, GST 160, total **1210**.
5. Make GST "Already included in item prices": the test bill shows it as included; total 1050.
6. Payment methods: add "JazzCash" (Wallet, reference required). Turn off all but one method; the last one cannot be turned off.
7. Receipt design: add an Invoice template in English; the preview shows the logo, header, footer, tax lines. A second Invoice template in English is refused; Urdu is accepted.
8. Document numbers: change the invoice prefix to `FBR-`, save. "Next number" shows `FBR-000001`. Pick another branch: it has its own rules.
9. Modules: switch off Delivery and Inventory and save. The menu hides Delivery, Inventory and Suppliers at once. Apply "Cafe": the type changes and only Inventory stays on. Switch Delivery on by hand: the screen says the switches differ from the defaults; "Reset to defaults" puts them back. The audit log shows the old and new values.
10. A user without `Settings.Manage` can open the screens but the server refuses every save.
