Aspects.Fody
============

Aspect oriented coding using Fody

There is a good chance that this project will fail. I learned a little about AOP using PostSharp and wanted to see how difficult it would be to do something similar in Fody.


## todo

### Method boundary aspect

- OnEntry - *complete*
- OnSuccess
- OnException
- OnExit

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


## Building


## License



## Credits

Parts of Aspects.Fody are taken from or based on:

- [MethodDecorator.Fody](https://github.com/Fody/MethodDecorator)
