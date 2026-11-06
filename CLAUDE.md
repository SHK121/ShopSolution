ShopSolution - Project Instructions



Project Overview

ShopSolution is a .NET solution with a separate Angular frontend.



The solution contains projects for:

* Shop.Api
* Shop.Application
* Shop.Domain
* Shop.Infrastructure



It also contains automated test projects.



Architecture Rules

* Keep business rules in the appropriate domain or application layer.
* Keep infrastructure concerns out of the domain layer.
* Do not introduce unnecessary dependencies between layers.
* Follow the existing project structure and conventions.
* Inspect existing implementations before proposing architectural changes.
* Avoid unrelated refactoring when implementing a feature.



Testing and Validation

* Run dotnet test .\\ShopSolution.slnx after relevant backend changes.
* Investigate test failures before claiming a task is complete.
* Do not remove or weaken tests merely to make the test suite pass.
* Report which validation commands were executed and their results.
* Never claim that a build or test succeeded unless it was actually executed and passed.



Change Safety

* Inspect the current Git status before modifying files.
* Preserve existing user changes.
* Do not discard, reset, or overwrite user changes without explicit permission.
* Keep changes focused on the requested task.
* Explain significant design decisions and trade-offs.



Working Environment

* The primary development environment is Windows.
* PowerShell may be used for automation.
* Use commands that are compatible with the active shell.
* Do not assume that the solution root is a Git repository; verify repository boundaries first.



## Definition of Done



A task is complete only when all applicable conditions are satisfied:



* The requested behavior is implemented.
* Changes are limited to the scope of the task.
* Existing architecture and dependency boundaries are preserved.
* Relevant automated tests pass.
* The solution builds successfully.
* The final report accurately describes what changed.
* The final report lists validation commands and their actual results.
* Any unverified behavior, failed check, or remaining limitation is explicitly reported.



If a test or build fails:



* Do not claim the task is complete.
* Inspect the failure and identify its likely cause.
* Fix the issue when it is within the requested scope.
* Rerun the relevant validation after making changes.
* Report unresolved failures honestly.



## Implementation Workflow



For each task:



1. Inspect the relevant source files and existing tests.
2. Check the current working-tree state before making changes.
3. Explain the intended approach when the change involves significant design decisions.
4. Implement the smallest reasonable changes.
5. Run relevant tests and build validation.
6. Review the resulting diff for unintended changes.
7. Summarize the implementation, validation results, and remaining risks.



Never overwrite, discard, or revert existing user changes without explicit permission.


