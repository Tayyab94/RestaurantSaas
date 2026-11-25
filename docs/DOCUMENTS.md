# Project documents (17 October 2026, notes added 22 November 2026)

## Files in this folder (Word and Excel; the Markdown sources are in `src/`)

| File | What it is | Version |
|---|---|---|
| `FRS.docx` | Functional Requirements Specification: every function of every module (307 requirements) | 1.0 |
| `Module_Status.docx` | How many modules are built, status by phase and by module | 1.0 |
| `Data_Dictionary_and_Requirements.xlsx` | All 236 tables and 2,057 columns, all choice lists, the permission matrix (roles by permissions) and the requirements register with live summary | 0.3 |
| `API_Specification.docx` | Every endpoint under `/api/v1`, access rules, paging, errors | 0.3 |
| `User_Guide.docx` | Guide for owners and managers, with a glossary | 0.3 |
| `Security_Overview.docx` | Security controls built and planned, rules for developers | 0.3 |
| `Release_Notes.docx` | What changed in versions 0.1 to 0.3 | 0.3 |

## Documents kept online (editable)

| Document | Version | Link |
|---|---|---|
| Software Requirements Specification (SRS) | 2.6 | https://claude.ai/artifact/Gstyz9xwDcB2N4FHe6YzZD |
| Functional Requirements Specification (FRS) | 1.0 | https://claude.ai/artifact/SCuMoSFJLg7QyqPVjwqUKU |
| Module Status and Progress Report | 1.0 | https://claude.ai/artifact/1G5FsPooDQHPZSffgiLEdD |
| Database Schema Design | 2.7 | https://claude.ai/artifact/JVFnDATzXWYezuUZmfzbuv |
| Technical Architecture | 1.4 | https://claude.ai/artifact/UfYhFhHeFKhahZA5y4s51V |
| UI/UX Specification | 1.1 | https://claude.ai/artifact/WD7pVsRvwdu93epXg84sUv |
| Test Plan and QA Strategy | 1.6 | https://claude.ai/artifact/B17kQRnQmMQXeQvVCtP8Ck |
| Deployment and Installation Guide | 1.3 | https://claude.ai/artifact/4x2QLyu4kHZ6vSvHVNmm4S |
| Project Plan and Risk Register | 2.2 | https://claude.ai/artifact/2dowCTvTpbPMcjUdUoVv9A |
| Development Flow and Module Build Sequence (Word, in the delivery folder) | 1.6 | Development_Flow_and_Module_Build_Sequence.docx |

Export the online documents to Word, PDF or Markdown from their page and keep the copies in this folder when a version is signed off.
When a module is finished: change the status words in `src/FRS.md`, rebuild the Word and Excel files, and update `Module_Status`.

## Documents inside the repository

| File | What it is |
|---|---|
| `CLAUDE.md` | Rules, code patterns, current status. **Read first in every session.** |
| `README.md` | How to run the solution, sign-in for development |
| `docs/M04-Licensing.md` | How licensing works, settings, manual checks |
| `docs/M05-Business-Settings.md` | How business settings work, assumptions, manual checks |

## Status note (22 November 2026)

`FRS.docx` and `Module_Status.docx` (version 1.0) still show M04 and M05 as not done. M04 (license key) is complete and M05 (business settings) is written. Section 13 of `CLAUDE.md` is the newer truth until those Word files are rebuilt. The next module is **M06 Menu Management**.
