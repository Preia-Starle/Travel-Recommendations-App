# Assignment7Rebuilt
Travel Recommendations app

Functionalities overview:

-	Authentication and autorisation: different access rights for unlogged in and logged in user
-	Logged User:
	- edit profile
	- add recommendation
	- delete recommendation
	- publish recommendation (=make it available to view in the feed on the main page for all users regardless of user status)
	- view my recommendation in My Recommendations section (thats where editing, deleting and publishing is avalaibale - UI updates based on status)
	- add/remove recommendation to/from favorites (toggle)
	- view all favorites in the Favorites section
	- add comment to published recommendations
	- log out
-	Unlogged user:
	- create profile
	- log in
	- only able to view published recommendations, needs to create a profile to unlock other options

Test scenarios:
Create a profile (click on Create profile and fill in the form details)
Log in with the created credentials
Create a recommendation (try as logged in and unlogged user)
Delete recommendation (click My recommendations view and delete by Delete button click}
Edit recommendation (click My recommendations view and edit by Edit button click – edit details in the form and save}
Publish recommendation(recommendation will be also displayed in the main page feed and button in the My Recommendations view will change to Unpublish, Delete, Edit options will hidden until recommendation unpublished)
View profile on View profile click on the navigation button
Edit profile(in the View profile mode click Edit and change information in the form, confirm by button click)
Save recommendation as favorite and unsave(toggle)
View saved favorites on Favorites navigation button click

ToDo:
Display Comments on UI for published recommendations
Printing options
Reflect changes straightaway on UI after Editing recommendation - now needs a refresh

