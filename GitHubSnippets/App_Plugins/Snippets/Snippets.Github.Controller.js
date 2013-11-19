angular.module("umbraco").controller("Snippets.GitHubController", function ($scope, snippetResource) {

    //Repository Name & User
    $scope.repoUser = snippetResource.getRepoUser();
    $scope.repo     = snippetResource.getRepoName();

    //Get Snippets from Resource (API)
    snippetResource.getSnippets('/Razor/Navi/Navi.cshtml').then(function (snippets) {
        $scope.snippets = snippets;
    });
    
    //Insert Snippet - button click
    $scope.insertSnippet = function () {

        var repo = "test";

        //Submit dialog - fires callback event for open dialog
        $scope.submit(repo);
    };
    
});
