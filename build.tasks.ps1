properties {
  $codecoverage = "VisualStudio2013"
  
  $msbuild_exe = Resolve-Path (${env:ProgramFiles(x86)} + "\MSBuild\14.0\Bin\MSBuild.exe")
  $vstest_console_exe = Resolve-Path ($env:VS140COMNTOOLS + "..\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe")
  if ($codecoverage -eq "VisualStudio2013") {
    $code_coverage_exe = Resolve-Path ($env:VS120COMNTOOLS + "..\..\Team Tools\Dynamic Code Coverage Tools\CodeCoverage.exe")
  }
  
  $test_results_dir = "TestResults\"
  
  $test_assembly_pattern = "test\**\*.Tests.dll"
  
  $sln_file = "Gesetzesentwicklung-Tool.sln"
  $coverage_xml = "coverage.coveragexml"
  
  $test_assemblies = @()
}

task default

task RestoreNuGetPackages {
  exec {
    nuget.exe restore $sln_file
  }
}

task Clean {
  if (Test-Path $test_results_dir) {
    Remove-Item $test_results_dir -r
  }
  
  if (Test-Path $coverage_xml) {
    Remove-Item $coverage_xml
  }
  
  exec { & $msbuild_exe "/t:Clean" $sln_file }
}

task Build -depends Clean, RestoreNuGetPackages {
  exec { & $msbuild_exe $sln_file }
}

task FindTestAssemblies {
  $script:test_assemblies = @(Get-ChildItem "test\**\*.Tests.dll" -Recurse)
}

task Test -depends FindTestAssemblies -ContinueOnError {
  $appveyor_logger = ""
  if (Test-Path Env:\APPVEYOR_BUILD_VERSION) {
    $appveyor_logger = "/Logger:Appveyor"
  }

  & $vstest_console_exe /InIsolation `
                        /EnableCodeCoverage `
                        /Settings:CodeCoverage.runsettings `
                        $appveyor_logger `
                        /TestAdapterPath:.\packages\NUnitTestAdapter.2.0.0\lib\ `
                        $script:test_assemblies
  
  if ($codecoverage -eq "VisualStudio2013") {
    $coverage_file = Resolve-Path -path "TestResults\*\*.coverage"
    Write-Host ("Analyzing " + $coverage_file + " and saving as " + $coverage_xml)
    & $code_coverage_exe analyze /output:$coverage_xml "$coverage_file"
  }
}

task ResolveCoveralls {
  $script:coveralls_exe = Resolve-Path "packages\coveralls.net.*\csmacnz.coveralls.exe"
}

task AppVeyor-PublishCodeCoverage -depends ResolveCoveralls {
  Write-Host $codecoverage
  
  if (!(Test-Path $coverage_xml)) {
    throw ("Code-Coverage-Datei fehlt: " + $coverage_xml)
  }
  
  & $script:coveralls_exe --dynamiccodecoverage `
                          -i coverage.coveragexml `
                          -o coverallsTestOutput.json `
                          --repoToken $env:coveralls_token `
                          --useRelativePaths `
                          --commitId $env:APPVEYOR_REPO_COMMIT `
                          --commitBranch $env:APPVEYOR_REPO_BRANCH `
                          --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR `
                          --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL `
                          --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE `
                          --jobId $env:APPVEYOR_JOB_ID
}

task AppVeyor-TestAndPublishCodeCoverage -depends Test, AppVeyor-PublishCodeCoverage