' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Threading
Imports Microsoft.CodeAnalysis.Editor.UnitTests.Extensions

Namespace Microsoft.VisualStudio.LanguageServices.UnitTests.CodeModel
    Public MustInherit Class AbstractCodeAttributeTests
        Inherits AbstractCodeElementTests(Of EnvDTE80.CodeAttribute2)

        Protected Overrides Function GetEndPointFunc(codeElement As EnvDTE80.CodeAttribute2) As Func(Of EnvDTE.vsCMPart, EnvDTE.TextPoint)
            Return Function(part) codeElement.GetEndPoint(part)
        End Function

        Protected Overrides Function GetFullName(codeElement As EnvDTE80.CodeAttribute2) As String
            Return codeElement.FullName
        End Function

        Protected Overrides Function GetKind(codeElement As EnvDTE80.CodeAttribute2) As EnvDTE.vsCMElement
            Return codeElement.Kind
        End Function

        Protected Overrides Function GetName(codeElement As EnvDTE80.CodeAttribute2) As String
            Return codeElement.Name
        End Function

        Protected Overrides Function GetParent(codeElement As EnvDTE80.CodeAttribute2) As Object
            Return codeElement.Parent
        End Function

        Protected Overrides Function GetStartPointFunc(codeElement As EnvDTE80.CodeAttribute2) As Func(Of EnvDTE.vsCMPart, EnvDTE.TextPoint)
            Return Function(part) codeElement.GetStartPoint(part)
        End Function

        Protected Overrides Function GetNameSetter(codeElement As EnvDTE80.CodeAttribute2) As Action(Of String)
            Return Sub(name) codeElement.Name = name
        End Function

        Protected Overridable Sub Delete(codeElement As EnvDTE80.CodeAttribute2)
            codeElement.Delete()
        End Sub

        Protected Overridable Function GetAttributeArguments(codeElement As EnvDTE80.CodeAttribute2) As EnvDTE.CodeElements
            Return codeElement.Arguments
        End Function

        Protected Overridable Function GetTarget(codeElement As EnvDTE80.CodeAttribute2) As String
            Return codeElement.Target
        End Function

        Protected Overridable Function GetValue(codeElement As EnvDTE80.CodeAttribute2) As String
            Return codeElement.Value
        End Function

        Protected Overridable Function AddAttributeArgument(codeElement As EnvDTE80.CodeAttribute2, data As AttributeArgumentData) As EnvDTE80.CodeAttributeArgument
            Return codeElement.AddArgument(data.Value, data.Name, data.Position)
        End Function

        Protected Sub TestAttributeArguments(code As XElement, ParamArray expectedAttributeArguments() As Action(Of Object))
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                Dim attributes = GetAttributeArguments(codeElement)
                Assert.Equal(expectedAttributeArguments.Length, attributes.Count)

                For i = 1 To attributes.Count
                    expectedAttributeArguments(i - 1)(attributes.Item(i))
                Next
            End Using
        End Sub

        Protected Sub TestTarget(code As XElement, expectedTarget As String)
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                Dim target = GetTarget(codeElement)
                Assert.Equal(expectedTarget, target)
            End Using
        End Sub

        Protected Sub TestSetTarget(code As XElement, expectedCode As XElement, target As String)
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeAttribute = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeAttribute)

                codeAttribute.Target = target

                Dim text = state.GetDocumentAtCursor().GetTextAsync(CancellationToken.None).Result.ToString()

                Assert.Equal(expectedCode.NormalizedValue().Trim(), text.Trim())
            End Using
        End Sub

        Protected Sub TestSetValue(code As XElement, expectedCode As XElement, value As String)
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeAttribute = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeAttribute)

                codeAttribute.Value = value

                Dim text = state.GetDocumentAtCursor().GetTextAsync(CancellationToken.None).Result.ToString()

                Assert.Equal(expectedCode.NormalizedValue().Trim(), text.Trim())
            End Using
        End Sub

        Protected Sub TestValue(code As XElement, expectedValue As String)
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                Dim target = GetValue(codeElement)
                Assert.Equal(expectedValue, target)
            End Using
        End Sub

        Protected Sub TestAddAttributeArgument(code As XElement, expectedCode As XElement, data As AttributeArgumentData)
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                Dim attributeArgument = AddAttributeArgument(codeElement, data)
                Assert.NotNull(attributeArgument)
                Assert.Equal(data.Name, attributeArgument.Name)

                Dim text = state.GetDocumentAtCursor().GetTextAsync(CancellationToken.None).Result.ToString()

                Assert.Equal(expectedCode.NormalizedValue.Trim(), text.Trim())
            End Using
        End Sub

        Protected Sub TestDelete(code As XElement, expectedCode As XElement)
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                codeElement.Delete()

                Dim text = state.GetDocumentAtCursor().GetTextAsync(CancellationToken.None).Result.ToString()

                Assert.Equal(expectedCode.NormalizedValue.Trim(), text.Trim())
            End Using
        End Sub

        Protected Sub TestDeleteAttributeArgument(code As XElement, expectedCode As XElement, indexToDelete As Integer)
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                Dim argument = CType(codeElement.Arguments.Item(indexToDelete), EnvDTE80.CodeAttributeArgument)

                argument.Delete()

                Dim text = state.GetDocumentAtCursor().GetTextAsync(CancellationToken.None).Result.ToString()

                Assert.Equal(expectedCode.NormalizedValue.Trim(), text.Trim())
            End Using
        End Sub

        Protected Function IsAttributeArgument(Optional name As String = Nothing, Optional value As String = Nothing) As Action(Of Object)
            Return _
                Sub(o)
                    Dim a = TryCast(o, EnvDTE80.CodeAttributeArgument)
                    Assert.NotNull(a)

                    If name IsNot Nothing Then
                        Assert.Equal(name, a.Name)
                    End If

                    If value IsNot Nothing Then
                        Assert.Equal(value, a.Value)
                    End If
                End Sub
        End Function

        Protected Sub TestAttributeArgumentStartPoint(code As XElement, index As Integer, ParamArray expectedParts() As Action(Of Func(Of EnvDTE.vsCMPart, EnvDTE.TextPoint)))
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                Dim arg = CType(codeElement.Arguments.Item(index), EnvDTE80.CodeAttributeArgument)

                Dim startPointGetter = Function(part As EnvDTE.vsCMPart) arg.GetStartPoint(part)

                For Each action In expectedParts
                    action(startPointGetter)
                Next
            End Using
        End Sub

        Protected Sub TestAttributeArgumentEndPoint(code As XElement, index As Integer, ParamArray expectedParts() As Action(Of Func(Of EnvDTE.vsCMPart, EnvDTE.TextPoint)))
            Using state = CreateCodeModelTestState(GetWorkspaceDefinition(code))
                Dim codeElement = state.GetCodeElementAtCursor(Of EnvDTE80.CodeAttribute2)()
                Assert.NotNull(codeElement)

                Dim arg = CType(codeElement.Arguments.Item(index), EnvDTE80.CodeAttributeArgument)

                Dim endPointGetter = Function(part As EnvDTE.vsCMPart) arg.GetEndPoint(part)

                For Each action In expectedParts
                    action(endPointGetter)
                Next
            End Using
        End Sub

    End Class
End Namespace

