// get this from the Console when login on dev.odyssey.ninja
// authToken =
//   'eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJQZW5PUXd2ZmNQTHgxNks3akdud0tWX29jS29Pd3JqMkVvTTVtU0pYNmpvIn0.eyJleHAiOjE2MzUwNjM5MjMsImlhdCI6MTYzNDgxNTQ4NCwiYXV0aF90aW1lIjoxNjM0ODA0NzIzLCJqdGkiOiJiNDUwNjdmMC1mYWU4LTRmNzktYTM1OC01NDZiMDNkNjBiYTMiLCJpc3MiOiJodHRwczovL2Rldi14NXU0MmRvLm9keXNzZXkubmluamEvYXV0aC9yZWFsbXMvTW9tZW50dW0iLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiYzg0YmNkNzItYTc3Zi00MmQwLTliNTUtMDhhMDM0ZDkzZTc2IiwidHlwIjoiQmVhcmVyIiwiYXpwIjoicmVhY3QtY2xpZW50Iiwibm9uY2UiOiJkZjdmNGE4Ny0yN2FmLTQ1OGEtOTQ5OS1jNzA0OGQxY2QyZGYiLCJzZXNzaW9uX3N0YXRlIjoiNWY5OGRjNTItOTI3NS00ZThlLWJkNjItN2ZmZWM4M2Q4MmIyIiwiYWNyIjoiMCIsImFsbG93ZWQtb3JpZ2lucyI6WyIqIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgZW1haWwgcHJvZmlsZSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJtcXR0Ijp7InRvcGljcyI6eyJyZWFkIjpbImludGVyYWN0aW9ucy9jODRiY2Q3Mi1hNzdmLTQyZDAtOWI1NS0wOGEwMzRkOTNlNzYvIyIsInVuaXR5LWFjdGlvbnMvYzg0YmNkNzItYTc3Zi00MmQwLTliNTUtMDhhMDM0ZDkzZTc2LyMiXSwid3JpdGUiOlsidXNlcnMvYzg0YmNkNzItYTc3Zi00MmQwLTliNTUtMDhhMDM0ZDkzZTc2LyMiLCJzcGFjZXMvIyIsInN0cnVjdHVyZS8jIiwiY29udHJvbC9saXZlc3RyZWFtL3NldCIsImludGVyYWN0aW9ucy8rL2M4NGJjZDcyLWE3N2YtNDJkMC05YjU1LTA4YTAzNGQ5M2U3NiJdfX0sIm5hbWUiOiJLYW1lbiBEaW1pdHJvdiIsInByZWZlcnJlZF91c2VybmFtZSI6ImthbWVuQGluY2luZXJhdGlvbi5nYW1lcyIsImdpdmVuX25hbWUiOiJLYW1lbiIsImZhbWlseV9uYW1lIjoiRGltaXRyb3YiLCJlbWFpbCI6ImthbWVuQGluY2luZXJhdGlvbi5nYW1lcyJ9.f7FbNzPSFRd_5ZiF2ySbuK0-6z8LMv9CCW8bDsXcevQg96l0zQxEjeTIp5UglYBpF1iIP-sB0VhY4PcdQQb7Z5FVC5D7B1biqRaU9dnVNhTbrkS3z7IZAzY6XcPsdkh8mNmR4fRtyT6HkRvhLd8hCiXq3h6K3rF1jcTzw-Ax53azHIraiGiS9phbi7lvrLuoAl-q9_gL8EfwsY4HxRf1A3G0_wU8P-Yu1RmttdAkk67YS6sd4KvbmRRkJHZVrSZWM3QFBUz5_oxXnD9UWd5glb5eRSFrRL9Wot7uNoPHfqOhzJFBlPDnkuuhekD2bjcLB7643LGENja5qUxOFJI8VA';
authToken =
  'eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJQZW5PUXd2ZmNQTHgxNks3akdud0tWX29jS29Pd3JqMkVvTTVtU0pYNmpvIn0.eyJleHAiOjE2Mzg3OTQxODAsImlhdCI6MTYzODUzNTI2NywiYXV0aF90aW1lIjoxNjM4NTM0OTgwLCJqdGkiOiJlMjM0MGU1YS1hZGRjLTQ5MDItODg2NC1iMGY3YTg1YmMyNDUiLCJpc3MiOiJodHRwczovL2Rldi14NXU0MmRvLm9keXNzZXkubmluamEvYXV0aC9yZWFsbXMvTW9tZW50dW0iLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiN2JiZDhkNDQtMzZiNi00NmM1LTlkMWItOWFhMzkwYzkxNGMzIiwidHlwIjoiQmVhcmVyIiwiYXpwIjoicmVhY3QtY2xpZW50Iiwibm9uY2UiOiI3ODdkNDIxNy0wNzcxLTRlMTctOWI2ZS1iYTMyNjU3ZGQxYzgiLCJzZXNzaW9uX3N0YXRlIjoiMDBlYmI4MTgtZTU0Zi00MmE5LTk4MGEtNzcwMDRjOGU2YTIzIiwiYWNyIjoiMCIsImFsbG93ZWQtb3JpZ2lucyI6WyIqIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJvcGVuaWQgZW1haWwgcHJvZmlsZSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJtcXR0Ijp7InRvcGljcyI6eyJyZWFkIjpbImludGVyYWN0aW9ucy83YmJkOGQ0NC0zNmI2LTQ2YzUtOWQxYi05YWEzOTBjOTE0YzMvIyIsInVuaXR5LWFjdGlvbnMvN2JiZDhkNDQtMzZiNi00NmM1LTlkMWItOWFhMzkwYzkxNGMzLyMiXSwid3JpdGUiOlsidXNlcnMvN2JiZDhkNDQtMzZiNi00NmM1LTlkMWItOWFhMzkwYzkxNGMzLyMiLCJzcGFjZXMvIyIsInN0cnVjdHVyZS8jIiwiY29udHJvbC9saXZlc3RyZWFtL3NldCIsImludGVyYWN0aW9ucy8rLzdiYmQ4ZDQ0LTM2YjYtNDZjNS05ZDFiLTlhYTM5MGM5MTRjMyJdfX0sIm5hbWUiOiJIZXJtYW4gTGVkZXJlciIsInByZWZlcnJlZF91c2VybmFtZSI6Imdlcm1hbnMubGVkZXJlcnNAZ21haWwuY29tIiwiZ2l2ZW5fbmFtZSI6Ikhlcm1hbiIsImZhbWlseV9uYW1lIjoiTGVkZXJlciIsImVtYWlsIjoiZ2VybWFucy5sZWRlcmVyc0BnbWFpbC5jb20ifQ.ZFSl4H9uk1dn2w4XUl_TVfKC4KUMCWoIeVENbJ54dBs3z8wB8bUWORk8GCDsivErylnLpHG_9HPslcfQZRJJCjFmQfeT1oXi5Rf3Xqbq5JPJ-TdmSKWXWDQUtowg3dGRsP2lsJGW2GAH3yhTzawvEvmc0eEZySGput8VAtt8nlYqy6Kxt7u3Mf9q7EVfUxsm7DnNx4Op8jNnxGBTo9_qwwLXbVh3IFdEWGlQbALTiygiGyGVSH7AMMloGCd1evtlyzszXC-WvG7BTZKV3M9rllcEwDwajjFF6DtKR4eCtN9CPgTe59NoOEgxczeHrW4NGVR5wteEfamKKa9ta3HeMQ';