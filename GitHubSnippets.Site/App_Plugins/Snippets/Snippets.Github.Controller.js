angular.module("umbraco").controller("Snippets.GitHubController", function ($scope, snippetResource) {

    //Repository Name & User
    $scope.repoUser = snippetResource.getRepoUser();
    $scope.repo     = snippetResource.getRepoName();

    //Get Snippets from Resource (API)
    snippetResource.getSnippets('/').then(function (snippets) {
        $scope.snippets = snippets;
    });
    
    //Insert Snippet - button click
    $scope.insertSnippet = function (selectedSnippet) {
        
        var selectedSnippetPath = selectedSnippet.path;

        //Get the snippet to decode from the Resource aka API
        snippetResource.getSnippetDecoded(selectedSnippetPath).then(function (snip) {
            
            //Create a snippet object to pass through to callback
            var snippet = {
                name: selectedSnippet.name,
                code: snip.data
            };

            //Debugging
            console.log(snippet);

            //Submit dialog - fires callback event for open dialog
            $scope.submit(snippet);
        });

    };
    
});
