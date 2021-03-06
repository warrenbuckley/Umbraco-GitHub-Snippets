﻿angular.module("umbraco.resources")
    .factory("snippetResource", function ($http) {
        return {
            getSnippets: function (path) {
                return $http.get("/umbraco/snippets/github/GetContent?path=" + path);
            },
            
            getSnippetDecoded: function (path) {
                return $http.get("/umbraco/snippets/github/GetContentDecoded?path=" + path);
            },

            getRepoUser: function () {
                return $http.get("/umbraco/snippets/github/GetRepositoryUser");
            },

            getRepoName: function () {
                return $http.get("/umbraco/snippets/github/GetRepositoryName");
            }
        };
});