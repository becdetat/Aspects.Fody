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
		public void Configure(IAspectsModuleContainer <col></col>ntainer, ModuleDefinition moduleDefinition) {
			var saveMethods = from type in moduleDefinition.Types
							  from method in type.Methods
							  where method.Name.StartsWith("Save")
							  select method
							  ;

			container.Apply<LoggingAspect>(saveMethods);
		}
	}

## Here's some IL
Because I needed to drop it somewhere.

	.method public hidebysig instance void  SubjectMethod() cil managed
	{
	  // Code size       58 (0x3a)
	  .maxstack  1
	  .locals init ([0] class TestWeaverTarget.MethodBoundary.OnSuccess.point_cut_is_inserted.SubjectOnSuccessAspect aspect)
	  IL_0000:  nop
	  IL_0001:  newobj     instance void TestWeaverTarget.MethodBoundary.OnSuccess.point_cut_is_inserted.SubjectOnSuccessAspect::.ctor()
	  IL_0006:  stloc.0
	  IL_0007:  ldloc.0
	  IL_0008:  callvirt   instance void [Aspects.Fody]Aspects.Fody.MethodBoundaryAspect::OnEntry()
	  IL_000d:  nop
	  .try
	  {
	    .try
	    {
	      IL_000e:  nop
	      IL_000f:  ldarg.0
	      IL_0010:  call       instance void TestWeaverTarget.MethodBoundary.OnSuccess.point_cut_is_inserted.Example::Payload()
	      IL_0015:  nop
	      IL_0016:  ldloc.0
	      IL_0017:  callvirt   instance void [Aspects.Fody]Aspects.Fody.MethodBoundaryAspect::OnSuccess()
	      IL_001c:  nop
	      IL_001d:  nop
	      IL_001e:  leave.s    IL_002b
	    }  // end .try
	    catch [mscorlib]System.Exception 
	    {
	      IL_0020:  pop
	      IL_0021:  nop
	      IL_0022:  ldloc.0
	      IL_0023:  callvirt   instance void [Aspects.Fody]Aspects.Fody.MethodBoundaryAspect::OnException()
	      IL_0028:  nop
	      IL_0029:  rethrow
	    }  // end handler
	    IL_002b:  nop
	    IL_002c:  leave.s    IL_0038
	  }  // end .try
	  finally
	  {
	    IL_002e:  nop
	    IL_002f:  ldloc.0
	    IL_0030:  callvirt   instance void [Aspects.Fody]Aspects.Fody.MethodBoundaryAspect::OnExit()
	    IL_0035:  nop
	    IL_0036:  nop
	    IL_0037:  endfinally
	  }  // end handler
	  IL_0038:  nop
	  IL_0039:  ret
	} // end of method Example::SubjectMethod




## Building
It should all build and tests should run (and hopefully pass). NuGet package restore is enabled. No gotchas at the moment. YMMV.


## License
See `LICENSE.txt`.


## Credits

Parts of Aspects.Fody are taken from or based on:

- [MethodDecorator.Fody](https://github.com/Fody/MethodDecorator)
