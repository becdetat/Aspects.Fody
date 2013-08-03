Aspects.Fody
============

Aspect oriented coding using Fody

There is a good chance that this project will fail. I'm learning about AOP using PostSharp at the moment and wanted to see how difficult it would be to do something similar in Fody.


## todo

### Method boundary aspect

- OnEntry cut point - *in progress*
- OnSuccess cut point
- OnException cut point
- OnExit cut point

Pseudo-code:

	OnEntry();
	try {
		Method();
		OnSuccess();
	} catch {
		OnException();
	} finally {
		OnExit();
	}


### Property boundary aspect

- OnGetEntry
- OnGetSuccess
- OnGetException
- OnGetExit
- OnSetEntry
- OnSetSuccess
- OnSetException
- OnSetExit

### Event boundary aspect

- OnAddHandler
- OnRemoveHandler
- OnRaiseHandlerEntry
- OnRaiseHandlerSuccess
- OnRaiseHandlerException
- OnRaiseHandlerExit


### Declarative configuration

Declarative configuration via an `IAspectsModule`, rather than the decorator approach:

	public class MyAspectsModule : IAspectsModule {
		public void Configure(IAspectsModuleContainer container, ModuleDefinition moduleDefinition) {
			var saveMethods = from type in moduleDefinition.Types
							  from method in type.Methods
							  where method.Name.StartsWith("Save")
							  select method
							  ;

			container.Apply<LoggingAspect>(saveMethods);
		}
	}


## Credits

Parts of Aspects.Fody are taken from or based on:

- [MethodDecorator.Fody](https://github.com/Fody/MethodDecorator)
